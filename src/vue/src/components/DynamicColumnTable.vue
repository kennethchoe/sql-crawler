<template>
  <v-data-table
    :headers="headers"
    :options.sync="options"
    :loading="loading || running"
    :items="results"
    :items-per-page="-1"
    item-key="ServerId"
    show-expand
    :hide-default-footer="true"
  >
    <template v-slot:item.data-table-expand="{ item, isExpanded, expand }">
      <v-icon
        @click="expand(true)"
        v-if="item.Error && !isExpanded"
        color="error"
        >mdi-alert-circle</v-icon
      >
      <v-icon
        @click="expand(false)"
        v-if="item.Error && isExpanded"
        color="error"
        >mdi-alert-circle-outline</v-icon
      >
    </template>
    <template v-slot:expanded-item="{ headers, item }">
      <error-info :headers="headers" :item="item" />
    </template>
    <template v-slot:item.Rows="{ value }">
      <v-card class="ma-4">
        <dynamic-column-table
          v-if="value && value.length"
          :results="value"
          :initial-headers="[]"
        />
      </v-card>
    </template>
    <template v-slot:progress>
      <v-progress-linear :indeterminate="loading" :value="loadingProgress" />
    </template>
  </v-data-table>
</template>

<script>
import ErrorInfo from "../components/ErrorInfo.vue";

export default {
  name: "dynamic-column-table",
  components: {
    ErrorInfo
  },
  props: {
    results: {
      type: Array,
      required: true
    },
    initialHeaders: {
      type: Array,
      required: true
    },
    loadingProgress: {
      type: Number,
      required: false,
      default: 0
    },
    loading: {
      type: Boolean,
      required: false,
      default: false
    },
    running: {
      type: Boolean,
      required: false,
      default: false
    }
  },
  data: () => ({
    loading: false,
    running: false,
    options: {}
  }),
  computed: {
    headers() {
      const result = this.initialHeaders;
      if (this.results.length > 0) {
        for (let i = 0; i < this.results.length; i++) {
          Object.keys(this.results[i]).forEach(c => {
            if (c === "Error") return;
            if (result.findIndex(x => x.text === c) >= 0) return;
            result.push({ text: c, value: c });
          });
        }
      }
      return result;
    }
  }
};
</script>
