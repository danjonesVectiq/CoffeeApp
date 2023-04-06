<script setup lang="ts">
import { useSearchStore } from '../stores/searchStore';


const searchStore = useSearchStore();
// const loading = ref<boolean>(false);
// defineProps<{ msg: string }>()
const loading = searchStore.loading();
const searchResults = searchStore.allSearchResults();


</script>

<template>
    <div class="results" :loading="loading">

    <template v-for="result in searchResults">
        <v-card v-if="result" :title="result.coffeeName" :subtitle="result.origin" text="Adipisicing laborum in consectetur anim."></v-card>
    </template>
    </div>
</template>

<style scoped></style>
