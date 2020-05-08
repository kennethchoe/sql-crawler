<template>
  <div>
    <v-card>
      <v-card-title>{{ queryName }}</v-card-title>
      <v-card-text
        ><pre class="pre-text">{{ queryBody }}</pre>
      </v-card-text>
      <v-card-actions>
        <v-btn text color="primary" @click="onRun" :loading="running"
          ><v-icon>mdi-play</v-icon>Run</v-btn
        >
        <v-btn
          text
          color="error"
          @click="onCancel"
          :style="{ visibility: running ? 'visible' : 'hidden' }"
          width="100"
          ><v-icon>mdi-stop</v-icon>Cancel</v-btn
        >
        <v-spacer />
        <div>{{ queryLastRetrievedAt }}</div>
      </v-card-actions>
    </v-card>
    <v-snackbar v-model="hasError" top :timeout="0">
      {{ errorMessage }}
      <v-dialog
        v-model="showErrorInfo"
        fullscreen
        hide-overlay
        transition="dialog-bottom-transition"
      >
        <template v-slot:activator="{ on }">
          <v-btn v-if="!!ErrorInfo" text v-on="on">More Info</v-btn>
        </template>
        <v-card>
          <v-toolbar dark color="primary">
            <v-btn icon dark @click="showErrorInfo = false">
              <v-icon>mdi-close</v-icon>
            </v-btn>
            <v-toolbar-title>More Info</v-toolbar-title>
          </v-toolbar>
          <pre class="pre-text">{{ ErrorInfo }}</pre>
        </v-card>
      </v-dialog>
      <v-btn text @click="hasError = false">Close </v-btn>
    </v-snackbar>
    <br />
    <dynamic-column-table
      :loading="loading"
      :running="running"
      :running-progress="queryProgress"
      :items="results"
      :initialHeaders="[
        { text: 'ServerId', value: 'ServerId' },
        { text: 'ServerName', value: 'ServerName' }
      ]"
    />
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";
import axios from "axios";
import DynamicColumnTable from "../components/DynamicColumnTable.vue";
import { toLocalString } from "./formatter";

export default {
  components: {
    DynamicColumnTable
  },
  props: {
    queryName: {
      type: String,
      required: true
    }
  },
  data: () => ({
    loading: false,
    running: false,
    hasError: false,
    errorMessage: "",
    showErrorInfo: false,
    ErrorInfo: "",
    cancellationSource: {}
  }),
  computed: {
    ...mapState(["results", "queries", "queryProgress"]),
    queryBody() {
      const query = this.queries.filter(x => x.name === this.queryName);
      return query.length ? query[0].query : "";
    },
    queryLastRetrievedAt() {
      const query = this.queries.filter(q => q.name === this.queryName);
      if (query.length) {
        if (query[0].lastRetrievedAtUtc)
          return (
            "Last retrieved at: " + toLocalString(query[0].lastRetrievedAtUtc)
          );
        else return "";
      }

      return "";
    }
  },
  methods: {
    ...mapActions(["getResults", "run", "ensureWeGotQueries"]),
    onRun() {
      const { CancelToken } = axios;
      this.cancellationSource = CancelToken.source();

      this.running = true;
      this.errorMessage = "";
      this.ErrorInfo = "";
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
          this.ErrorInfo = error.response ? error.response.data : "";
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
      this.ensureWeGotQueries()
        .then(() => {
          return this.getResults(this.queryName);
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
