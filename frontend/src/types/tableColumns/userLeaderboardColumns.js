import { createColumnHelper } from '@tanstack/vue-table'
import { h } from 'vue'
import { RouterLink } from 'vue-router'
import { formatDistanceToNow } from 'date-fns'

const columnHelper = createColumnHelper()
export const getUserLeaderboardColumns = () => [
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
