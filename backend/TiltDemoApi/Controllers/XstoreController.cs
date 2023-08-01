using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.AspNetCore.Mvc;
using TiltDemoApi.Kafka;
using TiltDemoApi.Xstore;

namespace TiltDemoApi.Controllers;

/*
 docker exec -it ksqldb-cli ksql http://ksqldb-server:8088
 SET 'auto.offset.reset' = 'earliest';
 create stream order_items (orderId int key) with (kafka_topic='order_items', key_format='kafka', value_format='json_sr');
 create table placed_orders (orderId int primary key) with (kafka_topic='placed_orders', key_format='kafka', value_format='json_sr', timestamp='purchasedate', timestamp_format='yyyy-MM-dd''T''HH:mm:ss[.SSSSSSS][.SSSSSS][.SSSSS][.SSSS][.SSS][.SS][.S]xxx');
 create table placed_orders_part with (partitions=6, key_format='kafka', value_format='json_sr') as select * from placed_orders;
 select * from order_items left join placed_orders_part on order_items.orderid = placed_orders_part.orderid emit changes;
 create table user_orders as select po.userid, collect_list(map(cast(po.orderid as string) := STRUCT(PID:=OI.PRODUCTID, QTY:=OI.QUANTITY, PR:=OI.UNITPRICE))) orders from order_items oi left join placed_orders_part po on oi.orderid = po.orderid group by po.userid;
 
 create table user_orders_tmp with (key_format='delimited') as select po.userid, po.orderid, collect_list(STRUCT(PID:=OI.PRODUCTID, QTY:=OI.QUANTITY, PR:=OI.UNITPRICE)) orders from order_items oi left join placed_orders_part po on oi.orderid = po.orderid group by po.userid, po.orderid;
 create table user_orders2 as select userid, collect_list(struct(orderId := po_orderid, order_items := orders)) o from user_orders_tmp group by userid;
 
 create stream placed_orders_str (orderId int key) with (kafka_topic='placed_orders', key_format='kafka', value_format='json_sr', timestamp='purchasedate', timestamp_format='yyyy-MM-dd''T''HH:mm:ss[.SSSSSSS][.SSSSSS][.SSSSS][.SSSS][.SSS][.SS][.S]xxx');
 create stream placed_orders_str2 as select * from placed_orders_str partition by userid;
 select userid, from_unixtime(WINDOWSTART) as ws, from_unixtime(WINDOWEND) as we, from_unixtime(min(ROWTIME)) as rt, count(*) as c from placed_orders_str2 window tumbling (size 30 days, grace period 1 hour) group by userid emit changes;
 */

[ApiController]
[Route("[controller]")]
public class XstoreController : ControllerBase
{
    private readonly KafkaDependentProducer<int, User> _userProducer;
    private readonly KafkaDependentProducer<int, Product> _productProducer;
    private readonly KafkaDependentProducer<int, PlacedOrder> _placedOrderProducer;
    private readonly KafkaDependentProducer<int, OrderItem> _orderItemProducer;
    private readonly KafkaDependentAdmin _admin;

    private static int usersCount = 5;
    private static int productsCount = 10;

    public XstoreController(KafkaDependentProducer<int, User> userProducer, KafkaDependentProducer<int, Product> productProducer,
        KafkaDependentProducer<int, PlacedOrder> placedOrderProducer, KafkaDependentProducer<int, OrderItem> orderItemProducer, KafkaDependentAdmin admin)
    {
        _userProducer = userProducer;
        _productProducer = productProducer;
        _placedOrderProducer = placedOrderProducer;
        _orderItemProducer = orderItemProducer;
        _admin = admin;
    }

    [HttpGet("admin")]
    public async Task<IActionResult> Admin()
    {
        await _admin.Client.CreateTopicsAsync(new []
        {
            new TopicSpecification(){Name = "products", NumPartitions = 2, ReplicationFactor = 1},
            new TopicSpecification(){Name = "users", NumPartitions = 2, ReplicationFactor = 1},
            new TopicSpecification(){Name = "order_items", NumPartitions = 6, ReplicationFactor = 1},
            new TopicSpecification(){Name = "placed_orders", NumPartitions = 4, ReplicationFactor = 1},
            new TopicSpecification(){Name = "canceled_orders", NumPartitions = 4, ReplicationFactor = 1},
        });

        return Ok("ok");
    }
    
    [HttpGet("setup")]
    public async Task<IActionResult> Setup()
    {
        for (var i = 0; i < usersCount; ++i)
        {
            var u = new User() { Id = i, Name = $"user-{i}" };
            await _userProducer.ProduceAsync("users", new Message<int, User>(){Key = u.Id, Value = u});
        }
        for (var i = 0; i < productsCount; ++i)
        {
            var u = new Product() { Id = i, Name = $"product-{i}" };
            await _productProducer.ProduceAsync("products", new Message<int, Product>(){Key = u.Id, Value = u});
        }
        return Ok("ok");
    }
    
    [HttpGet("oi/{oid}/{pid}/{qty}/{pr}")]
    public async Task<IActionResult> AddOrderItem(int oid, int pid, int qty, decimal pr)
    {
        var oi = new OrderItem();
        oi.OrderId = oid;
        oi.ProductId = pid;
        oi.Quantity = qty;
        oi.UnitPrice = pr;
        await _orderItemProducer.ProduceAsync("order_items",
            new Message<int, OrderItem>() { Key = oi.OrderId, Value = oi });
        return Ok(oi);
    }
    
    [HttpGet("setupplaced")]
    public async Task<IActionResult> SetupPlacedOrders()
    {
        var rnd = new Random();
        var oid = 0;
        for (var i = 0; i < usersCount; ++i)
        {
            var oc = rnd.Next(0, 10);
            for (var j = 0; j < oc; ++j)
            {
                var ts = new TimeSpan(rnd.Next(0, 12) * 30, rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60));
                var dt = DateTime.Now - ts;
                var order = new PlacedOrder()
                    { OrderId = oid++, UserId = i, PurchaseDate = dt };
                var oic = rnd.Next(1, productsCount + 1);
                var l = Enumerable.Repeat(false, productsCount).ToList();
                for (var k = 0; k < oic; ++k)
                {
                    var oi = new OrderItem();
                    var pk = rnd.Next(0, productsCount - k);
                    var y = 0;
                    for (var t = 0; t < l.Count; ++t)
                    {
                        if (!l[t] && y < pk)
                            ++y;
                        else if (!l[t] && y == pk)
                        {
                            y = t;
                            break;
                        }
                    }

                    l[y] = true;
                    oi.OrderId = order.OrderId;
                    oi.ProductId = y;
                    oi.Quantity = rnd.Next(1, 11);
                    oi.UnitPrice = y + 1;
                    await _orderItemProducer.ProduceAsync("order_items",
                        new Message<int, OrderItem>() { Key = oi.OrderId, Value = oi });
                }

                // var time = Confluent.Kafka.Timestamp.DateTimeToUnixTimestampMs(order.PurchaseDate);
                await _placedOrderProducer.ProduceAsync("placed_orders",
                    new Message<int, PlacedOrder>() { Key = order.OrderId, Value = order, Timestamp = new Timestamp(order.PurchaseDate, TimestampType.CreateTime)});
            }
        }
        
        return Ok("ok");
    }
}