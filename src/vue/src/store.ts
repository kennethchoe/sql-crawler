import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";

Vue.use(Vuex);

export default new Vuex.Store({
  state: () => ({
    liveness: false,
    servers: [],
    queries: [],
    results: [],
    settings: []
  }),
  mutations: {
    SET_LIVENESS(state, value) {
      state.liveness = value;
    },
    SET_SERVERS(state, servers) {
      state.servers = servers;
    },
    SET_QUERIES(state, queries) {
      state.queries = queries;
    },
    SET_RESULTS(state, results) {
      state.results = results;
    },
    SET_SETTINGS(state, settings) {
      state.settings = settings;
    }
  },
  actions: {
    getLiveness(context) {
      return axios.get("/api/monitoring/liveness").then(r => {
        context.commit("SET_LIVENESS", r.data);
      });
    },
    getServers(context) {
      return axios.get("/api/servers").then(r => {
        context.commit("SET_SERVERS", r.data);
      });
    },
    getQueries(context) {
      return axios.get("/api/sqlQueries").then(r => {
        context.commit("SET_QUERIES", r.data);
      });
    },
    run(context, { queryName, cancellationSource }) {
      return axios
        .post(
          `/api/sqlQueries/${queryName}/run`,
          {},
          { cancelToken: cancellationSource.token }
        )
        .then(() => {
          return context.dispatch("getResults", queryName);
        });
    },
    getResults(context, queryName) {
      return axios.get(`/api/sqlQueries/${queryName}`).then(r => {
        context.commit("SET_RESULTS", r.data);
      });
    },
    getSettings(context) {
      return axios.get(`/api/settings`).then(r => {
        context.commit("SET_SETTINGS", r.data);
      });
    }
  },
  modules: {}
});
