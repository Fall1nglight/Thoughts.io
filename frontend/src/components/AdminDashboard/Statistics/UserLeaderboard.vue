<script setup>
import { useAdminStats } from '@/stores/adminStats.js'
import { storeToRefs } from 'pinia'
import { h, onBeforeMount, ref, watchEffect } from 'vue'
import { statSortTypes } from '@/types/sort.types.js'
import {
  createColumnHelper,
  FlexRender,
  getCoreRowModel,
  getPaginationRowModel,
  useVueTable,
} from '@tanstack/vue-table'
import { RouterLink } from 'vue-router'
import { formatDistanceToNow } from 'date-fns'

// Dependencies
const adminStats = useAdminStats()
const { rankings } = storeToRefs(adminStats)

// Local state
const INITIAL_PAGE_INDEX = 0
const goToPageNumber = ref(INITIAL_PAGE_INDEX + 1)
const pageSizes = [1, 2, 3, 5, 10, 20, 30, 40, 50]
const columnHelper = createColumnHelper()
const columns = [
  columnHelper.accessor('username', {
    header: () => 'Username',
    cell: (info) =>
      h(
        RouterLink,
        {
          to: {
            name: 'profile',
            params: {
              userId: info.row.original.id,
            },
          },
          class: 'text-light',
        },
        () => info.getValue(),
      ),
  }),

  columnHelper.accessor('stats.thoughtCount', {
    header: () => 'Thoughts',
    cell: (info) => info.getValue(),
  }),

  columnHelper.accessor('stats.commentCount', {
    header: () => 'Comments',
    cell: (info) => info.getValue(),
  }),

  columnHelper.accessor('stats.reactions', {
    header: () => 'Reactions',
    cell: (info) => info.getValue().reduce((acc, curr) => (acc += curr.count), 0),
  }),

  columnHelper.accessor('createdAtUtc', {
    header: () => 'Member since',
    cell: (info) => formatDistanceToNow(new Date(info.getValue() + 'Z'), { addSuffix: false }),
  }),
]
const table = useVueTable({
  get data() {
    return rankings.value.users
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

// Hooks and watchers
onBeforeMount(() => {
  if (!rankings.value.userQuery.limit)
    rankings.value.userQuery.limit = statSortTypes.limitOptions[2]

  if (!rankings.value.userQuery.sortBy)
    rankings.value.userQuery.sortBy = statSortTypes.sortByOptions[0]
})

watchEffect(async () => {
  const limit = rankings.value.userQuery.limit
  if (limit) await adminStats.fetchUserLeaderboard(limit)
})
</script>

<template>
  <!-- Query options -->
  <div class="row pt-5">
    <div class="col-6">
      <span class="lead pe-3">Show top</span>
      <select v-model="rankings.userQuery.limit">
        <option v-for="limit of statSortTypes.limitOptions" :key="limit" :value="limit">
          {{ limit }}
        </option>
      </select>
    </div>

    <div class="col-6">
      <span class="lead pe-3">Sort by</span>
      <select v-model="rankings.userQuery.sortBy">
        <option v-for="sortType of statSortTypes.sortByOptions" :key="sortType" :value="sortType">
          {{ sortType }}
        </option>
      </select>
    </div>
  </div>

  <div class="row pt-5">
    <!-- Table -->
    <div class="col-12 table-responsive">
      <table class="table table-dark table-striped">
        <thead>
          <tr v-for="headerGroup in table.getHeaderGroups()" :key="headerGroup.id">
            <th v-for="header in headerGroup.headers" :key="header.id" :colspan="header.colSpan">
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
            <td v-for="cell in row.getAllCells()" :key="cell.id">
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
