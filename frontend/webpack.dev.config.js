// const webpackConfig = require('./webpack.config');
const webpack = require('webpack');
const {JsonStatsPlugin} = require('./webpack-stats-plugin');

module.exports = {
    plugins: [
        new JsonStatsPlugin('dist/frontend/stats.json')
    ]
};