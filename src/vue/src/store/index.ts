import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";

Vue.use(Vuex);

export default new Vuex.Store({
  state: () => ({
    liveness: false
  }),
  mutations: {
    SET_LIVENESS(state, value) {
      state.liveness = value;
    }
  },
  actions: {
    getLiveness(context) {
      return axios.get("/api/monitoring/liveness").then(r => {
        context.commit("SET_LIVENESS", r.data);
      });
    }
  },
  modules: {}
});
