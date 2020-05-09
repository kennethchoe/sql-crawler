import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";
import { sqlQueryInfo } from "./cs-core/SqlQueryInfo";

Vue.use(Vuex);

export default new Vuex.Store({
  state: () => ({
    liveness: false,
    servers: [],
    queries: [] as sqlQueryInfo[],
    queryProgress: 0,
    results: [],
    resultsByServer: [],
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
    SET_QUERY(state, query) {
      if (!query) return;

      const queryFiltered = state.queries.filter(q => q.name === query.name);
      if (queryFiltered.length) {
        state.queries.splice(state.queries.indexOf(queryFiltered[0]), 1, query);
      }
    },
    SET_QUERY_PROGRESS(state, value) {
      state.queryProgress = value;
    },
    SET_RESULTS(state, results) {
      state.results = results;
    },
    SET_RESULTS_BY_SERVER(state, results) {
      state.resultsByServer = results;
    },
    SET_SETTINGS(state, settings) {
      state.settings = settings;
    }
  },
  actions: {
    getLiveness(context) {
      return axios.get("api/monitoring/liveness").then(r => {
        context.commit("SET_LIVENESS", r.data);
      });
    },
    getServers(context) {
      return axios.get("api/servers").then(r => {
        context.commit("SET_SERVERS", r.data);
      });
    },
    getResultsByServer(context, serverId) {
      return axios.get(`api/servers/${serverId}/result`).then(r => {
        context.commit("SET_RESULTS_BY_SERVER", r.data);
      });
    },
    getQueries(context) {
      return axios.get("api/sqlQueries").then(r => {
        context.commit("SET_QUERIES", r.data);
      });
    },
    run(context, { queryName, cancellationSource }) {
      context.commit("SET_QUERY_PROGRESS", 0);
      return axios
        .post(
          `api/sqlQueries/${queryName}/run`,
          {},
          {
            cancelToken: cancellationSource.token,
            onDownloadProgress: function(progressEvent) {
              context.commit(
                "SET_QUERY_PROGRESS",
                (progressEvent.loaded / progressEvent.total) * 100
              );
            }
          }
        )
        .then(() => {
          return context.dispatch("getResults", queryName);
        })
        .then(() => {
          return axios.get(`api/sqlQueries/${queryName}`);
        })
        .then(r => {
          context.commit("SET_QUERY", r.data);
        });
    },
    ensureWeGotQueries(context) {
      if (context.state.queries.length) return;

      return context.dispatch("getQueries");
    },
    getResults(context, queryName) {
      return axios.get(`api/sqlQueries/${queryName}/result`).then(r => {
        context.commit("SET_RESULTS", r.data);
      });
    },
    getSettings(context) {
      return axios.get(`api/settings`).then(r => {
        context.commit("SET_SETTINGS", r.data);
      });
    }
  },
  modules: {}
});
