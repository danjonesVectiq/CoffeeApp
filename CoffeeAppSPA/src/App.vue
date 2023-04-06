<script setup lang="ts">
import Search from './components/Search.vue';
import SearchResults from './components/SearchResults.vue';
</script>

<template>
  <Search/>
  <SearchResults/>
</template>

<style scoped>
.logo {
  height: 6em;
  padding: 1.5em;
  will-change: filter;
  transition: filter 300ms;
}
.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}
.logo.vue:hover {
  filter: drop-shadow(0 0 2em #42b883aa);
}
</style>
