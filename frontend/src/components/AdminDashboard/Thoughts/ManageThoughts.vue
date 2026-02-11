<script setup>
import ManageThoughtsForm from '@/components/AdminDashboard/Thoughts/ManageThoughtsForm.vue'
import ManageThoughtsContainer from '@/components/AdminDashboard/Thoughts/ManageThoughtsContainer.vue'
</script>

<template>
  <section class="thoughts-manager pt-5">
    <div class="row">
      <div class="col-12">
        <p class="lead">Thoughts</p>
      </div>
    </div>

    <ManageThoughtsForm></ManageThoughtsForm>
    <ManageThoughtsContainer></ManageThoughtsContainer>
  </section>
</template>

<style scoped></style>
