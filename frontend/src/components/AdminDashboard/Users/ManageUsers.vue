<script setup>
import ManageUsersForm from '@/components/AdminDashboard/Users/ManageUsersForm.vue'
import ManageUsersContainer from '@/components/AdminDashboard/Users/ManageUsersContainer.vue'
</script>

<template>
  <section class="user-manager pt-5">
    <div class="row">
      <div class="col-12">
        <p class="lead">Users</p>
      </div>
    </div>

    <ManageUsersForm></ManageUsersForm>
    <ManageUsersContainer></ManageUsersContainer>
  </section>
</template>

<style scoped></style>
