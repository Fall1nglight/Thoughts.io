<script setup>
const { table } = defineProps({
  table: Object,
})

const emits = defineEmits(['toggleColumnVisibility'])
</script>

<template>
  <!-- Column visibility -->
  <div class="dropdown">
    <button
      type="button"
      class="btn btn-primary dropdown-toggle"
      data-bs-toggle="dropdown"
      aria-expanded="false"
      data-bs-auto-close="outside"
    >
      Select columns
    </button>

    <div class="dropdown-menu p-4">
      <div v-for="column in table.getAllLeafColumns()" :key="column.id">
        <label>
          <input
            type="checkbox"
            :checked="column.getIsVisible()"
            @input="emits('toggleColumnVisibility', column)"
          />
          {{ column.id }}
        </label>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
