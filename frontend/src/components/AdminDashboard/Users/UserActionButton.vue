<script setup>
const emits = defineEmits(['delete'])

// Methods
function handleDelete() {
  if (!confirm('Are you sure you want to delete this user?')) return
  emits('delete')
}
</script>

<template>
  <button class="btn btn-outline-danger btn-sm" title="Delete" @click="handleDelete">
    <i class="fa-solid fa-trash-can"></i>
  </button>
</template>

<style scoped></style>
