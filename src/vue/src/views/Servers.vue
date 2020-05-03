<template>
  <div>
    <v-data-table
      :headers="headers"
      :options.sync="options"
      :loading="loading"
      :items="servers"
    >
      <!-- <template v-slot:item.serverId="{ item }">
        <router-link
          :to="{
            name: 'poll-by-server',
            params: { serverId: item.serverId, serverName: item.serverName }
          }"
        >
          {{ item.serverId }}</router-link
        >
      </template> -->
    </v-data-table>
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";

export default {
  data: () => ({
    loading: false,
    options: {
      sortBy: ["serverName"],
      sortDesc: [false]
    },
    headers: [
      { text: "ServerId", value: "serverId" },
      { text: "ServerName", value: "serverName" },
      { text: "Description", value: "description" },
      { text: "UserData1", value: "userData1" },
      { text: "UserData2", value: "userData2" }
    ]
  }),
  computed: {
    ...mapState(["servers"])
  },
  methods: {
    ...mapActions(["getServers"])
  },
  mounted() {
    this.loading = true;
    this.getServers().finally(() => {
      this.loading = false;
    });
  }
};
</script>
