import { Component } from '@angular/core';
import opentelemetry, { Exception, SpanStatusCode, ROOT_CONTEXT, createContextKey, Span, trace, Attributes, Counter } from "@opentelemetry/api";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'frontend';
  tracer;
  span: any = undefined;
  counter: Counter<Attributes>;
  clientid;

  constructor() {
    this.tracer = opentelemetry.trace.getTracer(
      'example-tracer2'
    );
    const meter = opentelemetry.metrics.getMeter('example-meter');
    this.counter = meter.createCounter('front_ex_count');
    this.clientid = opentelemetry.context.active().getValue(createContextKey('clientid')) as string;
  }

  ngOnInit() {
    this.tracer.startActiveSpan('init', span => {
      span.addEvent('oninit');
      this.enhanceWithClientId(span);
      span.end();
      this.span = span;
    });
  }

  delay(sec: number) {
    return new Promise((res) => {
      setTimeout(() => res(undefined), sec * 1000);
    });
  }

  enhanceWithClientId(s: Span){
    // let clientid = opentelemetry.context.active().getValue(createContextKey('clientid')) as string;
    s.setAttribute('clientId', this.clientid);
  }

  apiCall<T>(name: string, fn: () => T): T {
    let ss: any;
    this.tracer.startActiveSpan(`api link to frontend for ${name}`, span => {
      span.end();
      ss = span;
    });

    let s = this.tracer.startSpan(`api call ${name}`, { links: [{ context: ss!.spanContext() }] }, ROOT_CONTEXT);
    let c = opentelemetry.trace.setSpan(ROOT_CONTEXT, s);

    this.tracer.startActiveSpan(`frontend link to api for ${name}`, { links: [{ context: s.spanContext() }] }, span => {
      span.end();
    });

    return opentelemetry.context.with(c, () => {
      s.end();
      return fn()
    });
  }

  checkResponse(r: Response) {
    return r.ok ? Promise.resolve(r) : Promise.reject('fetch failed');
  }

  sth() {
    this.tracer.startActiveSpan('sth', span => {
      this.enhanceWithClientId(span);
      span.end();
    });
  }

  sth2() {
    this.tracer.startActiveSpan('sth2', span => {
      this.enhanceWithClientId(span);
      span.end();
    });
  }

  help() {
    const activeSpan = opentelemetry.trace.getActiveSpan();
    alert(`traceId: ${activeSpan?.spanContext().traceId}`);
  }

  ex() {
    this.tracer.startActiveSpan('before local ex', span => {
      this.enhanceWithClientId(span);
      span.addEvent('fetch');

      try {
        throw 'very bad exception';
      }
      catch (ex) {
        this.tracer.startActiveSpan('local ex', span => {
          this.enhanceWithClientId(span);
          span.recordException(ex as Exception);
          span.setStatus({ code: SpanStatusCode.ERROR });
          span.end();
        });
        this.counter.add(1, {clientId: this.clientid});
      }

      span.end();
    });
  }

  bla() {
    this.tracer.startActiveSpan('bla', span => {
      this.enhanceWithClientId(span);
      span.addEvent('fetch');

      let apic = () => this.delay(4).then(() => fetch('__BACK1_URL__/WeatherForecast/bla'));
      this.apiCall('bla', apic).then(this.checkResponse).then(r => r.text()).then(v => {
        this.tracer.startActiveSpan('success', span => {
          this.enhanceWithClientId(span);
          span.end();
        });
      }).catch(err => {
        this.tracer.startActiveSpan('errorbla', span => {
          this.enhanceWithClientId(span);
          span.recordException(err as Exception);
          span.setStatus({ code: SpanStatusCode.ERROR });
          span.end();
        });
        this.counter.add(1, {clientId: this.clientid});
      });

      span.end();
    });
  }

  bla2() {
    this.tracer.startActiveSpan('bla2', span => {
      this.enhanceWithClientId(span);
      span.addEvent('fetch');

      let apic = () => this.delay(4).then(() => fetch('__BACK1_URL__/WeatherForecast/bla2'));
      this.apiCall('bla2', apic).then(this.checkResponse).then(r => r.text()).then(v => {
        this.tracer.startActiveSpan('success2', span => {
          this.enhanceWithClientId(span);
          span.addEvent('fetchsuccess');
          span.end();
        });
      }).catch(err => {
        this.tracer.startActiveSpan('errorbla2', span => {
          this.enhanceWithClientId(span);
          span.recordException(err as Exception);
          span.setStatus({ code: SpanStatusCode.ERROR });
          span.end();
        });
        this.counter.add(1, {clientId: this.clientid});
      });

      span.end();
    });
  }

  msg() {
    this.tracer.startActiveSpan('msg', span => {
      this.enhanceWithClientId(span);
      span.addEvent('fetch');

      let a = Math.ceil(Math.random() * 100);
      let apic = () => this.delay(5).then(() => fetch('__BACK1_URL__/WeatherForecast/user/' + a));
      this.apiCall('msg', apic).then(this.checkResponse).then(r => r.text()).then(v => {
        this.tracer.startActiveSpan('successmsg', span => {
          this.enhanceWithClientId(span);
          span.addEvent('fetchsuccess');
          span.end();
        });
      }).catch(err => {
        this.tracer.startActiveSpan('errormsg', span => {
          this.enhanceWithClientId(span);
          span.recordException(err as Exception);
          span.setStatus({ code: SpanStatusCode.ERROR });
          span.end();
        });
        this.counter.add(1, {clientId: this.clientid});
      });

      span.end();
    });
  }

  delayedMsg() {
    this.tracer.startActiveSpan('delayedMsg', span => {
      this.enhanceWithClientId(span);
      span.addEvent('fetch');

      let a = Math.ceil(Math.random() * 100);
      let apic = () => this.delay(5).then(() => fetch('__BACK1_URL__/WeatherForecast/userdelay/10/' + a));
      this.apiCall('delayedMsg', apic).then(this.checkResponse).then(r => r.text()).then(v => {
        this.tracer.startActiveSpan('successdelaymsg', span => {
          this.enhanceWithClientId(span);
          span.addEvent('fetchsuccess');
          span.end();
        });
      }).catch(err => {
        this.tracer.startActiveSpan('errordelaymsg', span => {
          this.enhanceWithClientId(span);
          span.recordException(err as Exception);
          span.setStatus({ code: SpanStatusCode.ERROR });
          span.end();
        });
        this.counter.add(1, {clientId: this.clientid});
      });

      span.end();
    });
  }

  exl() {
    this.tracer.startActiveSpan('exl', span => {
      this.enhanceWithClientId(span);
      span.addEvent('fetch');

      let apic = () => this.delay(3).then(() => fetch('__BACK1_URL__/WeatherForecast/exl'));
      this.apiCall('exl', apic).then(this.checkResponse).then(r => r.text()).then(v => {
        this.tracer.startActiveSpan('successexl', span => {
          this.enhanceWithClientId(span);
          span.addEvent('fetchsuccessexl');
          span.end();
        });
      }).catch(err => {
        this.tracer.startActiveSpan('errorexl', span => {
          this.enhanceWithClientId(span);
          span.recordException(err as Exception);
          span.setStatus({ code: SpanStatusCode.ERROR });
          span.end();
        });
        this.counter.add(1, {clientId: this.clientid});
      });

      span.end();
    });
  }

  exr() {
    this.tracer.startActiveSpan('exr', span => {
      this.enhanceWithClientId(span);
      span.addEvent('fetch', { sessionId: 'someId' });

      let apic = () => this.delay(2).then(() => fetch('__BACK1_URL__/WeatherForecast/exr'));
      this.apiCall('exr', apic).then(this.checkResponse).then(r => r.text()).then(v => {
        this.tracer.startActiveSpan('successexr', span => {
          this.enhanceWithClientId(span);
          span.addEvent('fetchsuccessexr');
          span.end();
        });
      }).catch(err => {
        this.tracer.startActiveSpan('errorexr', span => {
          this.enhanceWithClientId(span);
          span.recordException(err as Exception);
          span.setStatus({ code: SpanStatusCode.ERROR });
          span.end();
        });
        this.counter.add(1, {clientId: this.clientid});
      });

      span.end();
    });
  }

  over() {
    this.tracer.startActiveSpan('over', span => {
      this.enhanceWithClientId(span);
      span.addEvent('over');
      span.end();
    });
  }
}
