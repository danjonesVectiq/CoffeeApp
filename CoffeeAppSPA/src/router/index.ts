import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  // history: createWebHistory(import.meta.env.BASE_URL),
  history: createWebHistory(),
  routes: [
    // {
    //   path: '/',
    //   redirect: '/assign'
    // },
  ],
});

// router.replace("/assign");

export default router;