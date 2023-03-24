using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiltDemoApi.Migrations
{
    public partial class cdc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("EXEC sys.sp_cdc_enable_db");

            migrationBuilder.Sql("EXEC sys.sp_cdc_enable_table @source_schema = N'dbo', @source_name = N'FirstData', @role_name = null, @filegroup_name = null, @supports_net_changes = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
