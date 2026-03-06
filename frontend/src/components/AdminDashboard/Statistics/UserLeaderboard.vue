<script setup>
import { useAdminStats } from '@/stores/adminStats.js'
import { storeToRefs } from 'pinia'
import { watchEffect } from 'vue'
import { statSortTypes } from '@/types/sort.types.js'
import {
  FlexRender,
  getCoreRowModel,
  getPaginationRowModel,
  useVueTable,
} from '@tanstack/vue-table'
import TablePagination from '@/components/Table/TablePagination.vue'
import { getUserLeaderboardColumns } from '@/types/tableColumns/userLeaderboardColumns.js'

// Dependencies
const adminStats = useAdminStats()
const { rankings } = storeToRefs(adminStats)

// Local state
const table = useVueTable({
  get data() {
    return rankings.value.users
  },

  columns: getUserLeaderboardColumns(),
  getCoreRowModel: getCoreRowModel(),
  getPaginationRowModel: getPaginationRowModel(),
})

// Hooks and watchers
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
            <td v-for="cell in row.getAllCells()" :key="cell.id" class="text-break">
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
