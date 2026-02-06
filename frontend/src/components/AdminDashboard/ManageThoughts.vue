<script setup>
import ManageThoughtsForm from '@/components/AdminDashboard/ManageThoughtsForm.vue'
import ManageThoughtsContainer from '@/components/AdminDashboard/ManageThoughtsContainer.vue'
</script>

<template>
  <section class="thoughts-manager">
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
