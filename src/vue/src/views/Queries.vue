<template>
  <div>
    <v-data-table
      :headers="headers"
      :options.sync="options"
      :loading="loading"
      :items="queries"
      :items-per-page="-1"
      :hide-default-footer="true"
    >
      <template v-slot:item.name="{ item }">
        <router-link
          :to="{
            name: 'poll-by-query',
            params: { queryName: item.name }
          }"
        >
          {{ item.name }}</router-link
        >
      </template>
      <template v-slot:item.lastRetrievedAtUtc="{ value }">
        {{ toLocalString(value) }}
      </template>
    </v-data-table>
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";
import { toLocalString } from "./formatter";

export default {
  data: () => ({
    loading: false,
    options: {
      sortBy: ["name"],
      sortDesc: [false]
    },
    headers: [
      { text: "Name", value: "name" },
      { text: "Last Retrieved At", value: "lastRetrievedAtUtc" }
    ]
  }),
  computed: {
    ...mapState(["queries"])
  },
  methods: {
    ...mapActions(["getQueries"]),
    toLocalString: toLocalString
  },
  mounted() {
    this.loading = true;
    this.getQueries().finally(() => {
      this.loading = false;
    });
  }
};
</script>
