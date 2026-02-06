<script setup>
import { computed, onMounted, provide, ref, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useThoughtsStore } from '@/stores/thoughts.js'
import reactionTypes from '@/types/reaction.types.js'
import ThoughtCommentsTab from '@/components/ThoughtCommentsTab.vue'
import ThoughtReactionsTab from '@/components/ThoughtReactionsTab.vue'

// Dependencies
const thoughtsStore = useThoughtsStore()
const { modal, focusedThought } = storeToRefs(thoughtsStore)

// Local state
const tabs = {
  comments: ThoughtCommentsTab,
  reactions: ThoughtReactionsTab,
}

const activeTab = ref('comments')
const activeReactionId = ref(0)

// Derived state
const commentsTabStyle = computed(() => (activeTab.value === 'comments' ? 'active' : ''))

// Provide
provide('activeReactionId', activeReactionId)

// Methods
function setActiveTab(tab) {
  activeTab.value = tab
  activeReactionId.value = 0
}

function setActiveTabWithReactionId(tab, reactionId) {
  activeTab.value = tab
  activeReactionId.value = reactionId
}

function getIconStyle(reactionId) {
  return reactionTypes[reactionId]
}

function isReactionActive(reactionId) {
  return activeReactionId.value === reactionId
}

// Hooks and watchers
onMounted(() => {
  if (window.bootstrap) {
    if (!modal.value) {
      const modalEl = document.getElementById('activity-modal')
      modal.value = new window.bootstrap.Modal(modalEl)

      modalEl.addEventListener('hide.bs.modal', () => {
        if (document.activeElement) document.activeElement.blur()
      })

      modalEl.addEventListener('hidden.bs.modal', () => {
        focusedThought.value = null
        activeTab.value = 'comments'
        activeReactionId.value = null
      })
    }
  } else {
    console.error('Failed to load bootstrap.')
  }
})

// todo | implement loader with composables
watch(focusedThought, async (newVal) => {
  if (newVal) modal.value.show()
  if (newVal?.id) {
    await thoughtsStore.fetchReactions(newVal.id)
    await thoughtsStore.fetchComments(newVal.id)
  }
})
</script>

<template>
  <div
    class="modal fade"
    id="activity-modal"
    tabindex="-1"
    aria-labelledby="activity-modal-label"
    aria-hidden="true"
  >
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable align-items-baseline">
      <div class="modal-content">
        <div class="modal-header">
          <button
            @click="setActiveTab('comments')"
            type="button"
            class="btn btn-dark"
            :class="commentsTabStyle"
          >
            Comments
          </button>

          <button
            v-for="reaction in focusedThought?.reactions"
            :key="reaction.id"
            @click="setActiveTabWithReactionId('reactions', reaction.id)"
            type="button"
            class="btn btn-dark mx-2"
            :class="{ active: isReactionActive(reaction.id) }"
          >
            <i :class="getIconStyle(reaction.id)"></i>
            <span>{{ reaction.count }}</span>
          </button>

          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body">
          <component v-if="focusedThought" :is="tabs[activeTab]"></component>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
