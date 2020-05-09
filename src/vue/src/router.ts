import Vue from "vue";
import VueRouter, { RouteConfig } from "vue-router";

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
  {
    path: "/",
    name: "Queries",
    component: () =>
      import(/* webpackChunkName: "queries" */ "./views/Queries.vue")
  },
  {
    path: "/servers",
    name: "Servers",
    component: () =>
      import(/* webpackChunkName: "servers" */ "./views/Servers.vue")
  },
  {
    path: "/settings",
    name: "Settings",
    component: () =>
      import(/* webpackChunkName: "settings" */ "./views/Settings.vue")
  },
  {
    path: "/poll-by-query/:queryName",
    name: "poll-by-query",
    props: true,
    component: () =>
      import(/* webpackChunkName: "poll-by-query" */ "./views/PollByQuery.vue")
  },
  {
    path: "/poll-by-server/:serverId",
    name: "poll-by-server",
    props: true,
    component: () =>
      import(
        /* webpackChunkName: "poll-by-server" */ "./views/PollByServer.vue"
      )
  }
];

const router = new VueRouter({
  routes
});

export default router;
