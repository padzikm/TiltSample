import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { Resource } from "@opentelemetry/resources";
import { SemanticResourceAttributes } from "@opentelemetry/semantic-conventions";
import { BatchSpanProcessor } from '@opentelemetry/sdk-trace-base';
import { WebTracerProvider } from '@opentelemetry/sdk-trace-web';
import { ZoneContextManager } from '@opentelemetry/context-zone-peer-dep';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import opentelemetry, {createContextKey} from "@opentelemetry/api";
import {
  ConsoleMetricExporter,
  MeterProvider,
  PeriodicExportingMetricReader
} from "@opentelemetry/sdk-metrics";
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-http';


const resource =
  Resource.default().merge(
    new Resource({
      [SemanticResourceAttributes.SERVICE_NAME]: "myfront",
      [SemanticResourceAttributes.SERVICE_VERSION]: "0.1.0",
    })
  );

const options = {
  resource,
}

const provider = new WebTracerProvider(options);
provider.addSpanProcessor(new BatchSpanProcessor(new OTLPTraceExporter()));

provider.register({
  // Changing default contextManager to use ZoneContextManager - supports asynchronous operations - optional
  contextManager: new ZoneContextManager(),
});

// Registering instrumentations
registerInstrumentations({
  instrumentations: [
    // new DocumentLoadInstrumentation(),
    // new UserInteractionInstrumentation({eventNames: ['click']}),
    new FetchInstrumentation()
  ],
});

const collectorOptions = {
  // url: '<opentelemetry-collector-url>', // url is optional and can be omitted - default is http://localhost:4318/v1/metrics
  // headers: {}, // an optional object containing custom headers to be sent with each request
  concurrencyLimit: 1, // an optional limit on pending requests
};
const metricExporter = new OTLPMetricExporter(collectorOptions);

const myServiceMeterProvider = new MeterProvider({
  resource: resource,
});

const metricReader = new PeriodicExportingMetricReader({
  exporter: metricExporter,

  // Default is 60000ms (60 seconds). Set to 3 seconds for demonstrative purposes only.
  exportIntervalMillis: 3000,
});

myServiceMeterProvider.addMetricReader(metricReader);

// Set this MeterProvider to be global to the app being instrumented.
opentelemetry.metrics.setGlobalMeterProvider(myServiceMeterProvider);

if (environment.production) {
  enableProdMode();
}

let tracer = opentelemetry.trace.getTracer(
  'example-tracer'
);

let id = Math.ceil(Math.random() * 100);
tracer.startActiveSpan('boot', s => {
  s.end();
  let key = createContextKey('clientid');
  let ctx = opentelemetry.context.active().setValue(key, `client-id-${id}`);
  opentelemetry.context.with(ctx, () => {
    platformBrowserDynamic().bootstrapModule(AppModule)
      .catch(err => console.error(err));
  });
});
