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

if (environment.production) {
  enableProdMode();
}

let tracer = opentelemetry.trace.getTracer(
  'example-tracer'
);

tracer.startActiveSpan('boot', s => {
  s.end();
  let key = createContextKey('clientid');
  let ctx = opentelemetry.context.active().setValue(key, 'some-client-id');
  opentelemetry.context.with(ctx, () => {
    platformBrowserDynamic().bootstrapModule(AppModule)
      .catch(err => console.error(err));
  });
});
