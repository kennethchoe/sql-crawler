<template>
  <v-app>
    <v-content>
      <div id="nav">
        <router-link to="/">Queries</router-link> |
        <router-link to="/servers">Servers</router-link>
      </div>
      <v-container>
        <v-row no-gutters>
          <v-col>
            <router-view />
          </v-col>
        </v-row>
      </v-container>
    </v-content>
  </v-app>
</template>

<style lang="scss">
#nav {
  padding: 30px;

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}
</style>
