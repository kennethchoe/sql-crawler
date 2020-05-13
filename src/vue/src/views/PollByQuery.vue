<template>
  <div>
    <v-card>
      <v-card-title>{{ queryName }}</v-card-title>
      <v-card-subtitle> {{ query.scope }}</v-card-subtitle>
      <v-card-text
        ><pre class="pre-text">{{ query.query }}</pre>
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
        <v-btn
          class="ma-2"
          fab
          v-if="hasRows"
          width="36"
          height="36"
          @click="flatten = !flatten"
          :color="flatten ? 'primary' : ''"
        >
          <v-icon>mdi-grid</v-icon>
        </v-btn>
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
      :items="resultsComputed"
      :item-key="itemKey"
      :initialHeaders="[
        { text: 'ServerId', value: 'ServerId' },
        { text: 'ServerName', value: 'ServerName' }
      ]"
      :props-to-hide="['QueryName']"
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
    flatten: false,
    running: false,
    hasError: false,
    errorMessage: "",
    showErrorInfo: false,
    ErrorInfo: "",
    cancellationSource: {}
  }),
  computed: {
    ...mapState(["results", "queries", "queryProgress"]),
    hasRows() {
      for (let i = 0; i < this.results.length; i++) {
        if (this.results[i]["Rows"]) return true;
      }
      return false;
    },
    itemKey() {
      if (!this.flatten) return "ServerId";
      return "rowId";
    },
    resultsComputed() {
      if (!this.flatten) return this.results;

      let rowId = 0;
      const computed = [];
      for (let i = 0; i < this.results.length; i++) {
        const row = { ...this.results[i], rowId };
        delete row["Rows"];

        if (this.results[i]["Rows"]) {
          const rowsParsed = this.results[i]["Rows"];
          for (let i = 0; i < rowsParsed.length; i++) {
            const rowInternal = { ...row, ...rowsParsed[i], rowId };
            computed.push(rowInternal);
            rowId = rowId + 1;
          }
        } else {
          computed.push(row);
          rowId = rowId + 1;
        }
      }

      return computed;
    },
    query() {
      const query = this.queries.filter(x => x.name === this.queryName);
      return query.length ? query[0] : {};
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
  white-space: pre-wrap;
  word-break: break-all;
}
</style>
