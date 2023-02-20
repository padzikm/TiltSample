import { createWriteStream, promises as fsPromises } from 'fs';
import { dirname } from 'path';
import { Compiler } from 'webpack';
import { Compilation, WebpackError } from 'webpack';
import assert from 'assert';
import { stringifyStream } from '@discoveryjs/json-ext';

export function assertIsError(value: unknown): asserts value is Error & { code?: string } {
  const isError =
    value instanceof Error ||
    // The following is needing to identify errors coming from RxJs.
    (typeof value === 'object' && value && 'name' in value && 'message' in value);
  assert(isError, 'catch clause variable is not an Error instance');
}

export function addWarning(compilation: Compilation, message: string): void {
  compilation.warnings.push(new WebpackError(message));
}

export function addError(compilation: Compilation, message: string): void {
  compilation.errors.push(new WebpackError(message));
}

export class JsonStatsPlugin {
  constructor(private readonly statsOutputPath: string) {}

  apply(compiler: Compiler) {
    compiler.hooks.done.tapPromise('unik-json-stats', async (stats) => {
    //   const { stringifyStream } = await import('@discoveryjs/json-ext');
      const data = stats.toJson('minimal');

      try {
        await fsPromises.mkdir(dirname(this.statsOutputPath), { recursive: true });
        await new Promise<void>((resolve, reject) =>
          stringifyStream(data)
            .pipe(createWriteStream(this.statsOutputPath))
            .on('close', resolve)
            .on('error', reject),
        );
      } catch (error) {
        assertIsError(error);
        addError(
          stats.compilation,
          `Unable to write stats file: ${error.message || 'unknown error'}`,
        );
      }
    });
  }
}