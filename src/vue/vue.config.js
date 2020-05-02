const port = process.env.VUE_APP_PORT || 51410;

module.exports = {
  transpileDependencies: [
    'vuetify',
  ],
  devServer: {
    proxy: {
      '/': {
        target: `http://localhost:${port}`,
      },
    },
  },
  productionSourceMap: false,
  outputDir: '../SqlCrawler.Web/wwwroot',
  chainWebpack: (config) => {
    config.module.rule('eslint').use('eslint-loader').options({
      fix: true,
    });
  },
};
