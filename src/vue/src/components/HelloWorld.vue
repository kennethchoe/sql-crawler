<template>
  <div>
    <h1>{{ msg }}</h1>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { mapActions } from "vuex";

@Component({
  methods: {
    ...mapActions(["getLiveness"])
  }
})
export default class HelloWorld extends Vue {
  @Prop() private msg!: string;
}
</script>

<style scoped lang="scss"></style>
