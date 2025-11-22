<script setup>
import ThoughtCardContainer from '@/components/ThoughtCardContainer.vue'
</script>

<template>
  <section class="welcome text-center p-5">
    <div class="row p-5">
      <h1>Welcome to Thoughts.io</h1>
      <h2>Share your public thoughts with the world or keep them private!</h2>
    </div>
  </section>

  <section class="publicThoughts p-5">
    <ThoughtCardContainer></ThoughtCardContainer>
  </section>
</template>

<style scoped>
.welcome > h1,
h2 {
  font-family: 'Poppins', sans-serif;
  font-weight: 300;
  font-style: normal;
}

.welcome > div {
  /* background-color: rgba(41, 44, 51, 0.5);*/
  background-color: rgba(255, 100, 21, 0.45);
  border-radius: 10px;
}

* {
  color: white;
}
</style>
