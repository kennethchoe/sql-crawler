<template>
  <div>
    <v-card>
      <v-card-title>{{ serverId }} - {{ server.serverName }}</v-card-title>
      <v-card-text>
        {{ server.description }}
      </v-card-text>
    </v-card>
    <br />
    <dynamic-column-table
      :loading="loading"
      :items="resultsByServer"
      item-key="QueryName"
      :initialHeaders="[{ text: 'QueryName', value: 'QueryName' }]"
      :props-to-hide="['ServerId']"
    />
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";
import DynamicColumnTable from "../components/DynamicColumnTable.vue";
import { toLocalString } from "./formatter";

export default {
  components: {
    DynamicColumnTable
  },
  props: {
    serverId: {
      type: String,
      required: true
    }
  },
  data: () => ({
    loading: false
  }),
  computed: {
    ...mapState(["resultsByServer", "servers"]),
    server() {
      const server = this.servers.filter(x => x.serverId === this.serverId);
      return server.length ? server[0] : {};
    }
  },
  methods: {
    ...mapActions(["getResultsByServer", "ensureWeGotServers"]),
    init() {
      this.loading = true;
      this.ensureWeGotServers()
        .then(() => {
          return this.getResultsByServer(this.serverId);
        })
        .finally(() => {
          this.loading = false;
        });
    }
  },
  mounted() {
    this.init();
  },
  watch: {
    serverId() {
      this.init();
    }
  }
};
</script>
