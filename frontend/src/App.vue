<script setup>
import { RouterView } from 'vue-router'
import NavBar from '@/components/NavBar.vue'
import ErrorModal from '@/components/ErrorModal.vue'
import ThoughtActivityModal from '@/components/ThoughtActivityModal.vue'
</script>

<template>
  <ErrorModal></ErrorModal>
  <ThoughtActivityModal></ThoughtActivityModal>

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
