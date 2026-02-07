<script setup>
import { computed, h, ref } from 'vue'
import {
  FlexRender,
  getCoreRowModel,
  getPaginationRowModel,
  useVueTable,
  createColumnHelper,
} from '@tanstack/vue-table'
import { format } from 'date-fns'
import { useAdminThoughtsStore } from '@/stores/adminThoughts.js'
import { storeToRefs } from 'pinia'
import ThoughtActionButtons from '@/components/AdminDashboard/Thoughts/ThoughtActionButtons.vue'

// Dependencies
const adminThoughts = useAdminThoughtsStore()
const { getThoughtsBySearch, focusedThought } = storeToRefs(adminThoughts)

// Local state

// search
const searchByOptions = {
  title: 'title',
  content: 'content',
  username: 'username',
}
const searchQuery = ref('')
const searchBy = ref(searchByOptions.title)
const filteredThoughts = computed(() => {
  if (searchBy.value === searchByOptions.title)
    return getThoughtsBySearch.value.filter((x) =>
      x.title.toLowerCase().includes(searchQuery.value.toLowerCase()),
    )

  if (searchBy.value === searchByOptions.content)
    return getThoughtsBySearch.value.filter((x) =>
      x.content.toLowerCase().includes(searchQuery.value.toLowerCase()),
    )

  if (searchBy.value === searchByOptions.username)
    return getThoughtsBySearch.value.filter((x) =>
      x.user.username.toLowerCase().includes(searchQuery.value.toLowerCase()),
    )

  return getThoughtsBySearch.value
})

const columnHelper = createColumnHelper()
const columnVisibility = ref({
  id: false,
  user_id: false,
  createdAtUtc: false,
  updatedAtUtc: false,
})
const INITIAL_PAGE_INDEX = 0
const goToPageNumber = ref(INITIAL_PAGE_INDEX + 1)
const pageSizes = [1, 2, 3, 5, 10, 20, 30, 40, 50]

const columns = [
  columnHelper.accessor('id', {
    header: () => 'Thought_Id',
    cell: (info) => info.getValue(),
  }),

  columnHelper.group({
    header: 'Author',
    columns: [
      columnHelper.accessor('user.id', {
        header: () => 'User_Id',
        cell: (info) => info.getValue(),
      }),

      columnHelper.accessor('user.username', {
        header: () => 'Username',
        cell: (info) => info.getValue(),
      }),
    ],
  }),

  columnHelper.group({
    header: 'Content',
    columns: [
      columnHelper.accessor('title', {
        cell: (info) => info.getValue(),
      }),

      columnHelper.accessor('content', {
        cell: (info) => info.getValue(),
      }),

      columnHelper.accessor('isPublic', {
        header: () => 'Visibility',
        cell: (info) => (info.getValue() ? 'Public' : 'Private'),
      }),
    ],
  }),

  columnHelper.group({
    header: 'Interactions',
    columns: [
      columnHelper.accessor('comments.count', {
        cell: (info) => info.getValue(),
        header: () => 'Comments',
      }),

      columnHelper.accessor('reactions', {
        cell: (info) => info.getValue().reduce((acc, curr) => (acc += curr.count), 0),
        header: () => 'Reactions',
      }),
    ],
  }),

  columnHelper.group({
    header: 'Timestamps',
    columns: [
      columnHelper.accessor('createdAtUtc', {
        header: () => 'Created',
        cell: (info) => format(info.getValue() + 'Z', 'yyyy.MM.dd (HH:mm:ss)'),
      }),

      columnHelper.accessor('updatedAtUtc', {
        header: () => 'Updated',
        cell: (info) =>
          new Date(info.getValue()).getUTCFullYear() > 1
            ? format(info.getValue() + 'Z', 'yyyy.MM.dd (HH:mm:ss)')
            : 'Never',
      }),
    ],
  }),

  columnHelper.display({
    id: 'actions',
    header: () => 'Actions',
    cell: (info) =>
      h(ThoughtActionButtons, {
        onDelete: () => handleDelete(info.row.original.id),
        onMakePrivate: () => toggleVisibility(info.row.original.id, false),
        onMakePublic: () => toggleVisibility(info.row.original.id, true),
        onShowComments: () => showComments(info.row.original.id),
      }),
  }),
]

const table = useVueTable({
  get data() {
    return filteredThoughts.value
  },

  state: {
    get columnVisibility() {
      return columnVisibility.value
    },
  },

  columns,
  getCoreRowModel: getCoreRowModel(),
  getPaginationRowModel: getPaginationRowModel(),
})

// Methods
function toggleColumnVisibility(column) {
  columnVisibility.value = {
    ...columnVisibility.value,
    [column.id]: !column.getIsVisible(),
  }
}

function handleGoToPage(e) {
  const page = e.target.value ? Number(e.target.value) - 1 : 0
  goToPageNumber.value = page + 1
  table.setPageIndex(page)
}

function handlePageSizeChange(e) {
  table.setPageSize(Number(e.target.value))
}

async function toggleVisibility(id, visibility) {
  try {
    await adminThoughts.updateThought(id, visibility)
  } catch (error) {
    console.error(error)
  }
}

async function handleDelete(id) {
  try {
    await adminThoughts.deleteThought(id)
  } catch (error) {
    console.error(error)
  }
}

function showComments(id) {
  focusedThought.value = getThoughtsBySearch.value.find((x) => x.id === id)
}
</script>

<template>
  <div class="row mt-5" v-if="!getThoughtsBySearch.length">
    <p class="lead">No data</p>
  </div>

  <div class="row mt-5" v-else>
    <div class="col-12">
      <h5 class="text-light">Results</h5>
    </div>

    <div class="col-12 d-flex justify-content-between align-items-center">
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
                @input="toggleColumnVisibility(column)"
              />
              {{ column.id }}
            </label>
          </div>
        </div>
      </div>

      <!-- Search -->
      <div class="d-flex justify-content-between">
        <div class="input-group me-2">
          <input type="text" class="form-control" id="searchQueryInput" v-model="searchQuery" />
          <div class="input-group-text"><i class="fa-solid fa-magnifying-glass"></i></div>
        </div>

        <select class="form-select" v-model="searchBy">
          <option :value="searchByOptions.title">Search by title</option>
          <option :value="searchByOptions.content">Search by content</option>
          <option :value="searchByOptions.username">Search by username</option>
        </select>
      </div>
    </div>

    <!-- Table -->
    <div class="col-12 table-responsive">
      <table class="table table-dark table-striped">
        <thead>
          <tr v-for="headerGroup in table.getHeaderGroups()" :key="headerGroup.id">
            <th v-for="header in headerGroup.headers" :key="header.id" :colSpan="header.colSpan">
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
  </div>
</template>

<style scoped></style>
