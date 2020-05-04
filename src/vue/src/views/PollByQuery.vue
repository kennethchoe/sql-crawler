<template>
  <div>
    <v-card>
      <v-card-title>{{ queryName }}</v-card-title>
      <v-card-text
        ><pre class="pre-text">{{ queryBody }}</pre></v-card-text
      >
      <v-card-actions>
        <v-spacer />
        <v-btn color="primary" @click="onRun" :loading="running" width="100"
          ><v-icon>mdi-play</v-icon>Run</v-btn
        >
        <v-btn
          color="error"
          @click="onCancel"
          :style="{ visibility: running ? 'visible' : 'hidden' }"
          width="100"
          ><v-icon>mdi-stop</v-icon>Cancel</v-btn
        >
      </v-card-actions>
    </v-card>
    <v-snackbar v-model="hasError" top :timeout="0">
      {{ errorMessage }}
      <v-dialog
        v-model="showMoreInfo"
        fullscreen
        hide-overlay
        transition="dialog-bottom-transition"
      >
        <template v-slot:activator="{ on }">
          <v-btn v-if="!!moreInfo" text v-on="on">More Info</v-btn>
        </template>
        <v-card>
          <v-toolbar dark color="primary">
            <v-btn icon dark @click="showMoreInfo = false">
              <v-icon>mdi-close</v-icon>
            </v-btn>
            <v-toolbar-title>More Info</v-toolbar-title>
          </v-toolbar>
          <pre class="pre-text">{{ moreInfo }}</pre>
        </v-card>
      </v-dialog>
      <v-btn text @click="hasError = false">Close </v-btn>
    </v-snackbar>
    <br />
    <v-data-table
      :headers="headers"
      :options.sync="options"
      :loading="loading"
      :items="results"
      :items-per-page="-1"
      :hide-default-footer="true"
    />
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";
import axios from "axios";

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
    options: {},
    hasError: false,
    errorMessage: "",
    showMoreInfo: false,
    moreInfo: "",
    cancellationSource: {}
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
      const { CancelToken } = axios;
      this.cancellationSource = CancelToken.source();

      this.running = true;
      this.errorMessage = "";
      this.moreInfo = "";
      this.hasError = false;
      const promise = this.run({
        queryName: this.queryName,
        cancellationSource: this.cancellationSource
      });

      promise
        .catch(error => {
          if (error instanceof axios.Cancel) {
            return;
          }
          this.errorMessage = error.message;
          this.moreInfo = error.response ? error.response.data : "";
          this.hasError = true;
        })
        .finally(() => {
          this.running = false;
        });
    },
    onCancel() {
      this.cancellationSource.cancel();
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
.pre-text {
  white-space: pre;
  text-align: left;
  overflow: auto;
}
</style>
