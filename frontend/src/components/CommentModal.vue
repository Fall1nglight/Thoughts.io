<script setup>
import { computed, h, onMounted, ref, watch } from 'vue'
import { useAdminThoughtsStore } from '@/stores/adminThoughts.js'
import { storeToRefs } from 'pinia'
import {
  FlexRender,
  getCoreRowModel,
  getPaginationRowModel,
  useVueTable,
  createColumnHelper,
} from '@tanstack/vue-table'

// Dependencies
const adminThoughts = useAdminThoughtsStore()
const { modal, focusedThought, focusedComments } = storeToRefs(adminThoughts)

// Local state
const searchByOptions = {
  content: 'content',
  username: 'username',
}
const searchQuery = ref('')
const searchBy = ref(searchByOptions.content)
const filteredComments = computed(() => {
  if (searchBy.value === searchByOptions.content)
    return focusedComments.value.filter((x) =>
      x.content.toLowerCase().includes(searchQuery.value.toLowerCase()),
    )

  if (searchBy.value === searchByOptions.username)
    return focusedComments.value.filter((x) =>
      x.user.username.toLowerCase().includes(searchQuery.value.toLowerCase()),
    )

  return focusedComments.value
})

const columnHelper = createColumnHelper()
const INITIAL_PAGE_INDEX = 0
const goToPageNumber = ref(INITIAL_PAGE_INDEX + 1)
const pageSizes = [1, 2, 3, 5, 10, 20, 30, 40, 50]

const columns = [
  columnHelper.accessor('user.username', {
    cell: (info) => info.getValue(),
    header: () => 'Username',
  }),

  columnHelper.accessor('content', {
    cell: (info) => info.getValue(),
    header: () => 'Comment',
  }),

  columnHelper.display({
    id: 'actions',
    header: () => 'Actions',
    cell: (info) =>
      h(
        'button',
        {
          class: 'btn btn-outline-danger btn-sm d-flex align-items-center justify-content-center',
          title: 'Delete thought',
          onClick: () => handleDelete(info.row.original.id),
        },
        [h('i', { class: 'fa-solid fa-trash-can' })],
      ),
  }),
]

const table = useVueTable({
  get data() {
    return filteredComments.value
  },

  columns,
  getCoreRowModel: getCoreRowModel(),
  getPaginationRowModel: getPaginationRowModel(),
})

// Methods
function handleGoToPage(e) {
  const page = e.target.value ? Number(e.target.value) - 1 : 0
  goToPageNumber.value = page + 1
  table.setPageIndex(page)
}

function handlePageSizeChange(e) {
  table.setPageSize(Number(e.target.value))
}

async function handleDelete(id) {
  if (!confirm('Are you sure you want to delete this comment?')) return

  try {
    await adminThoughts.deleteComment(focusedThought.value?.id, id)
  } catch (error) {
    console.error(error)
  }
}

// Hooks and watchers
onMounted(() => {
  if (window.bootstrap) {
    if (!modal.value) {
      const modalEl = document.getElementById('commentModal')
      modal.value = new window.bootstrap.Modal(modalEl)

      modalEl.addEventListener('hide.bs.modal', () => {
        if (document.activeElement) document.activeElement.blur()
      })

      modalEl.addEventListener('hidden.bs.modal', () => {
        focusedThought.value = null
      })
    }
  } else {
    console.error('Failed to load bootstrap.')
  }
})

watch(focusedThought, async (newVal) => {
  if (newVal) modal.value.show()

  if (newVal?.id) {
    await adminThoughts.fetchComments(newVal.id)
  }
})
</script>

<template>
  <div
    class="modal fade"
    id="commentModal"
    tabindex="-1"
    aria-labelledby="commentModalLabel"
    aria-hidden="true"
  >
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h1 class="modal-title fs-5" id="commentModalLabel">Comments</h1>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>

        <div class="modal-body" v-if="!focusedComments.length">
          <p class="lead">No comments found for this thought.</p>
        </div>

        <div class="modal-body" v-else>
          <!-- Search -->
          <div class="d-flex justify-content-between">
            <div class="input-group me-2">
              <input type="text" class="form-control" id="searchQueryInput" v-model="searchQuery" />
              <div class="input-group-text"><i class="fa-solid fa-magnifying-glass"></i></div>
            </div>

            <select class="form-select" v-model="searchBy">
              <option selected :value="searchByOptions.content">Search by content</option>
              <option :value="searchByOptions.username">Search by username</option>
            </select>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-striped">
              <thead>
                <tr v-for="headerGroup in table.getHeaderGroups()" :key="headerGroup.id">
                  <th
                    v-for="header in headerGroup.headers"
                    :key="header.id"
                    :colSpan="header.colSpan"
                  >
                    <FlexRender
                      v-if="!header.isPlaceholder"
                      :render="header.column.columnDef.header"
                      :props="header.getContext()"
                    />
                  </th>
                </tr>
              </thead>

              <tbody>
                <tr v-for="row in table.getRowModel().rows" :key="row.id">
                  <td v-for="cell in row.getVisibleCells()" :key="cell.id" class="text-break">
                    <FlexRender :render="cell.column.columnDef.cell" :props="cell.getContext()" />
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Pagination -->
          <div class="col-12 d-flex flex-column d-none">
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

            <div class="page-size-selector d-flex justify-content-between align-items-center mt-2">
              <span class="d-flex flex-column gap-1">
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
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
