<template>
  <div>
    <v-data-table
      :headers="headers"
      :options.sync="options"
      :loading="loading"
      :items="queries"
    >
      <template v-slot:item.name="{ item }">
        <router-link
          :to="{
            name: 'poll-by-query',
            params: { name: item.name }
          }"
        >
          {{ item.name }}</router-link
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
      sortBy: ["name"],
      sortDesc: [false]
    },
    headers: [
      { text: "Name", value: "name" },
      { text: "Query", value: "query", align: "start" }
    ]
  }),
  computed: {
    ...mapState(["queries"])
  },
  methods: {
    ...mapActions(["getQueries"])
  },
  mounted() {
    this.loading = true;
    this.getQueries().finally(() => {
      this.loading = false;
    });
  }
};
</script>
