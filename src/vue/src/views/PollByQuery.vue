<template>
  <div>
    <v-card>
      <v-card-title>{{ queryName }}</v-card-title>
      <v-card-text
        ><pre class="query-body">{{ queryBody }}</pre></v-card-text
      >
      <v-card-actions>
        <v-spacer />
        <v-btn @click="onRun" :loading="running">Run</v-btn>
      </v-card-actions>
    </v-card>
    <br />
    <v-data-table
      :headers="headers"
      :options.sync="options"
      :loading="loading"
      :items="results"
    />
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";

export default {
  props: {
    queryName: {
      type: String,
      required: true
    }
  },
  data: () => ({
    loading: false,
    running: false,
    options: {}
  }),
  computed: {
    ...mapState(["results", "queries"]),
    headers() {
      const result = [{ text: "ServerId", value: "ServerId" }];
      if (this.results.length > 0) {
        for (let i = 0; i < this.results.length; i++) {
          Object.keys(this.results[i]).forEach(c => {
            if (result.findIndex(x => x.text === c) >= 0) return;
            result.push({ text: c, value: c });
          });
        }
      }
      return result;
    },
    queryBody() {
      return this.queries.filter(x => x.name === this.queryName)[0].query;
    }
  },
  methods: {
    ...mapActions(["getResults", "run"]),
    onRun() {
      this.running = true;
      this.run(this.queryName).finally(() => {
        this.running = false;
      });
    },
    init() {
      this.loading = true;
      this.getResults(this.queryName).finally(() => {
        this.loading = false;
      });
    }
  },
  mounted() {
    this.init();
  },
  watch: {
    queryName() {
      this.init();
    }
  }
};
</script>

<style scoped>
.query-body {
  white-space: pre;
  text-align: left;
}
</style>
