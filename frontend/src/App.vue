<script setup>
import 'bootstrap/dist/js/bootstrap.bundle.js'
import { RouterView } from 'vue-router'
import NavBar from '@/components/NavBar.vue'
</script>

<template>
  <header>
    <NavBar></NavBar>
  </header>

  <main>
    <RouterView />
  </main>
</template>

<style>
@import 'bootswatch/dist/lux/bootstrap.min.css';
</style>
