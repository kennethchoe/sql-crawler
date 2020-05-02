<template>
  <div>
    {{ queries }}
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";

export default {
  computed: {
    ...mapState(["queries"])
  },
  methods: {
    ...mapActions(["getQueries"])
  },
  mounted() {
    this.getQueries();
  }
};
</script>
