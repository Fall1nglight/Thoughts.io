<script setup>
import { computed, h, ref } from 'vue'
import { storeToRefs } from 'pinia'
import {
  createColumnHelper,
  FlexRender,
  getCoreRowModel,
  getPaginationRowModel,
  useVueTable,
} from '@tanstack/vue-table'
import { useAdminUserStore } from '@/stores/adminUsers.js'
import UserActionButton from '@/components/AdminDashboard/Users/UserActionButton.vue'
import { useAuthStore } from '@/stores/auth.js'
import TablePagination from '@/components/Table/TablePagination.vue'
import TableColumnVisibility from '@/components/Table/TableColumnVisibility.vue'
import { getManageUserColumns } from '@/types/tableColumns/manageUserColumns.js'

// Dependencies
const adminUsers = useAdminUserStore()
const authStore = useAuthStore()
const { getUsersBySearch } = storeToRefs(adminUsers)
const { getUserId } = storeToRefs(authStore)

// Local state
const searchByOptions = {
  username: 'username',
}
const searchQuery = ref('')
const searchBy = ref(searchByOptions.username)
const filteredUsers = computed(() => {
  if (searchBy.value === searchByOptions.username)
    return getUsersBySearch.value.filter((x) =>
      x.username.toLowerCase().includes(searchQuery.value.toLowerCase()),
    )

  return getUsersBySearch.value
})

const columnHelper = createColumnHelper()
const columnVisibility = ref({
  id: false,
})

const columns = [
  ...getManageUserColumns(),

  columnHelper.display({
    id: 'actions',
    header: () => 'Actions',
    cell: (info) =>
      h(UserActionButton, {
        onDelete: () => handleDelete(info.row.original.id),
      }),
  }),
]

const table = useVueTable({
  get data() {
    return filteredUsers.value
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

async function handleDelete(id) {
  try {
    if (getUserId.value === id) {
      alert('You cannot delete your own account from the dashboard.')
      return
    }

    await adminUsers.deleteUser(id)
  } catch (error) {
    console.error(error)
  }
}
</script>

<template>
  <div class="row mt-5" v-if="!getUsersBySearch.length">
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
