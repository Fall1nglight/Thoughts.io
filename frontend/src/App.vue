<script setup>
import { RouterView } from 'vue-router'
import NavBar from '@/components/NavBar.vue'
import ErrorModal from '@/components/ErrorModal.vue'
import ThoughtActivityModal from '@/components/ThoughtActivityModal.vue'
import CommentModal from '@/components/CommentModal.vue'
</script>

<template>
  <ErrorModal></ErrorModal>
  <ThoughtActivityModal></ThoughtActivityModal>
  <CommentModal></CommentModal>

  <header>
    <NavBar></NavBar>
  </header>

  <main>
    <div class="container">
      <RouterView />
    </div>
  </main>
</template>

<style></style>
