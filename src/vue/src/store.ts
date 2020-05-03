import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";

Vue.use(Vuex);

export default new Vuex.Store({
  state: () => ({
    liveness: false,
    servers: [],
    queries: []
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
      return axios.get("/api/sqlSource").then(r => {
        context.commit("SET_QUERIES", r.data);
      });
    }
  },
  modules: {}
});
