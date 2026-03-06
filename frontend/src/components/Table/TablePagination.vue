<script setup>
import { ref } from 'vue'

const { table, INIT_PAGE_INDEX, pageSizes } = defineProps({
  table: Object,

  INIT_PAGE_INDEX: {
    type: Number,
    default: 0,
  },

  pageSizes: {
    type: Array,
    default: () => [1, 2, 3, 5, 10, 20, 30, 40, 50],
  },
})

// Local state
const goToPageNumber = ref(INIT_PAGE_INDEX + 1)

// Methods
function handleGoToPage(e) {
  const page = e.target.value ? Number(e.target.value) - 1 : 0
  goToPageNumber.value = page + 1
  table.setPageIndex(page)
}

function handlePageSizeChange(e) {
  table.setPageSize(Number(e.target.value))
}
</script>

<template>
  <div class="col-12 d-flex align-items-center justify-content-between">
    <div class="flex-grow-1" style="flex-basis: 0"></div>

    <div
      class="page-arrow-navigation d-flex align-items-center justify-content-center flex-grow-1"
      style="flex-basis: 0"
    >
      <div class="page-info">
        <div class="text-center mb-3">
          <span>
            <div>Page</div>
            <strong>
              {{ table.getState().pagination.pageIndex + 1 }} of
              {{ table.getPageCount() }}
            </strong>
          </span>
        </div>
        <div class="buttons d-flex gap-1">
          <button
            class="border rounded p-1"
            @click="() => table.setPageIndex(0)"
            :disabled="!table.getCanPreviousPage()"
          >
            <i class="fa-solid fa-angles-left"></i>
          </button>
          <button
            class="border rounded p-1"
            @click="() => table.previousPage()"
            :disabled="!table.getCanPreviousPage()"
          >
            <i class="fa-solid fa-angle-left"></i>
          </button>
          <button
            class="border rounded p-1"
            @click="() => table.nextPage()"
            :disabled="!table.getCanNextPage()"
          >
            <i class="fa-solid fa-angle-right"></i>
          </button>
          <button
            class="border rounded p-1"
            @click="() => table.setPageIndex(table.getPageCount() - 1)"
            :disabled="!table.getCanNextPage()"
          >
            <i class="fa-solid fa-angles-right"></i>
          </button>
        </div>
      </div>
    </div>

    <div
      class="page-size-selector d-flex align-items-center justify-content-end gap-1 flex-grow-1"
      style="flex-basis: 0"
    >
      <span class="d-flex align-items-center gap-1">
        Go to page:
        <input
          type="number"
          :value="goToPageNumber"
          @change="handleGoToPage"
          class="border p-1 rounded w-16"
        />
      </span>
      <select :value="table.getState().pagination.pageSize" @change="handlePageSizeChange">
        <option :key="pageSize" :value="pageSize" v-for="pageSize in pageSizes">
          Show {{ pageSize }}
        </option>
      </select>
    </div>
  </div>
</template>

<style scoped></style>
