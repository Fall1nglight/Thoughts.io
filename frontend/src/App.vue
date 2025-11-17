<script setup>
import { RouterView } from 'vue-router'
import NavBar from '@/components/NavBar.vue'
import ErrorModal from '@/components/ErrorModal.vue'
</script>

<template>
  <ErrorModal></ErrorModal>

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
