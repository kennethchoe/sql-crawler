<template>
  <div>
    {{ servers }}
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";

export default {
  computed: {
    ...mapState(["servers"])
  },
  methods: {
    ...mapActions(["getServers"])
  },
  mounted() {
    this.getServers();
  }
};
</script>
