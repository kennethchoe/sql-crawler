const port = process.env.VUE_APP_PORT || 51410;

module.exports = {
  transpileDependencies: [
    'vuetify',
  ],
  // https://github.com/vuetifyjs/vuetify/issues/5271#issuecomment-600031869
  css: {
    extract: { ignoreOrder: true },
  },
  devServer: {
    proxy: {
      '/': {
        target: `http://localhost:${port}`,
      },
    },
  },
  productionSourceMap: false,
  outputDir: '../SqlCrawler.Web/wwwroot',
  publicPath: process.env.WEBAPP_PUBLIC_PATH || '/',
  chainWebpack: (config) => {
    config.module.rule('eslint').use('eslint-loader').options({
      fix: true,
    });
  },
};
