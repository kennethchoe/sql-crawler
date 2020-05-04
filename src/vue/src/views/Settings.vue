<template>
  <div>
    <v-data-table
      :headers="headers"
      :loading="loading"
      :items="settings"
      :items-per-page="-1"
      :hide-default-footer="true"
    >
      <template v-slot:item.Value="{ value }">
        <div v-html="value" />
      </template>
    </v-data-table>
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";

export default {
  data: () => ({
    loading: false,
    headers: [
      { text: "Name", value: "Key" },
      { text: "Value", value: "Value" }
    ]
  }),
  computed: {
    ...mapState(["settings"])
  },
  methods: {
    ...mapActions(["getSettings"])
  },
  mounted() {
    this.loading = true;
    this.getSettings().finally(() => {
      this.loading = false;
    });
  }
};
</script>
