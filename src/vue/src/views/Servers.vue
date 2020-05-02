<template>
  <div>
    <v-data-table
      :headers="headers"
      :options.sync="options"
      :loading="loading"
      :items="servers"
    >
      <template v-slot:item.serverId="{ item }">
        <router-link
          :to="{
            name: 'view-server',
            params: { serverId: item.serverId, serverName: item.serverName }
          }"
        >
          {{ item.serverId }}</router-link
        >
      </template>
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
      { text: "Id", value: "serverId" },
      { text: "Name", value: "serverName" },
      { text: "Description", value: "description" }
    ],
    files: [],
    uploading: false,
    cancellationSource: {},
    deleting: false
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
