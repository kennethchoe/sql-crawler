import Vue from "vue";
import VueRouter, { RouteConfig } from "vue-router";
import Home from "./views/Home.vue";

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/servers",
    name: "Servers",
    component: () =>
      import(/* webpackChunkName: "servers" */ "./views/Servers.vue")
  },
  {
    path: "/queries",
    name: "Queries",
    component: () =>
      import(/* webpackChunkName: "queries" */ "./views/Queries.vue")
  }
];

const router = new VueRouter({
  routes
});

export default router;
