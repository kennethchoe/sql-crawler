<template>
  <div>
    <v-btn @click="onRun" :loading="running">Run</v-btn>
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
    ...mapState(["results"]),
    headers() {
      const result = [{ text: "ServerId", value: "ServerId" }];
      if (this.results.length > 0) {
        Object.keys(this.results[0]).forEach(c => {
          if (c === "ServerId") return;
          result.push({ text: c, value: c });
        });
      }
      return result;
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
