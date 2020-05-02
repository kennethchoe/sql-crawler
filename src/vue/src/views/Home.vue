<template>
  <div class="home">
    {{ liveness }}
  </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapActions } from "vuex";

export default {
  computed: {
    ...mapState(["liveness"])
  },
  methods: {
    ...mapActions(["getLiveness"])
  },
  mounted() {
    this.getLiveness();
  }
};
</script>
