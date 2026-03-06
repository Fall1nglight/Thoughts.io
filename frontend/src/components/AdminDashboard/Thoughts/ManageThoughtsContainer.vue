<script setup>
import { computed, h, ref } from 'vue'
import {
  FlexRender,
  getCoreRowModel,
  getPaginationRowModel,
  useVueTable,
  createColumnHelper,
} from '@tanstack/vue-table'
import { useAdminThoughtsStore } from '@/stores/adminThoughts.js'
import { storeToRefs } from 'pinia'
import ThoughtActionButtons from '@/components/AdminDashboard/Thoughts/ThoughtActionButtons.vue'
import TablePagination from '@/components/Table/TablePagination.vue'
import { getManageThoughtColumns } from '@/types/tableColumns/manageThoughtColumns.js'
import TableColumnVisibility from '@/components/Table/TableColumnVisibility.vue'

// Dependencies
const adminThoughts = useAdminThoughtsStore()
const { getThoughtsBySearch, focusedThought } = storeToRefs(adminThoughts)

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

const columns = [
  ...getManageThoughtColumns(),
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
      <TableColumnVisibility :table="table" @toggle-column-visibility="toggleColumnVisibility" />

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
    <TablePagination :table="table" />
  </div>
</template>

<style scoped></style>
