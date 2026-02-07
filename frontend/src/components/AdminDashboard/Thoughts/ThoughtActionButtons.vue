<script setup>
const emits = defineEmits(['delete', 'makePrivate', 'makePublic', 'showComments'])

// Methods
function handleDelete() {
  if (!confirm('Are you sure you want to delete this thought?')) return
  emits('delete')
}
</script>

<template>
  <div class="d-flex gap-2">
    <button class="btn btn-outline-danger btn-sm" title="Delete thought" @click="handleDelete">
      <i class="fa-solid fa-trash-can"></i>
    </button>

    <button
      class="btn btn-outline-warning btn-sm"
      title="Make private"
      @click="emits('makePrivate')"
    >
      <i class="fa-solid fa-lock"></i>
    </button>

    <button class="btn btn-outline-success btn-sm" title="Make public" @click="emits('makePublic')">
      <i class="fa-solid fa-lock-open"></i>
    </button>

    <button
      class="btn btn-outline-info btn-sm"
      title="Show comments"
      @click="emits('showComments')"
    >
      <i class="fa-solid fa-comment"></i>
    </button>
  </div>
</template>

<style scoped></style>
