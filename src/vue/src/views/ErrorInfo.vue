<template>
  <td :colspan="headers.length">
    <div class="more-info error--text ma-4">
      {{ item.Error }}
    </div>
  </td>
</template>

<script>
export default {
  props: ["headers", "item"]
};
</script>

<style scoped>
.fixed-width-table {
  width: 100%;
  table-layout: fixed;
}

.more-info-wrapper {
  max-height: 150px;
  overflow: scroll;
}
.more-info {
  color: red;
  white-space: pre-line;
}
</style>
