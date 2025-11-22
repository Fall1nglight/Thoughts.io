<script setup>
import { onMounted, provide, ref, watch } from 'vue'
import { useThoughtsStore } from '@/stores/thoughts.js'
import { storeToRefs } from 'pinia'
import reactionTypes from '@/types/reaction.types.js'
import ThoughtCommentsTab from '@/components/ThoughtCommentsTab.vue'
import ThoughtReactionsTab from '@/components/ThoughtReactionsTab.vue'

const thoughtsStore = useThoughtsStore()
const { modal, focusedThought } = storeToRefs(thoughtsStore)

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

watch(focusedThought, (newVal) => {
  if (newVal) modal.value.show()
})

const tabs = {
  comments: ThoughtCommentsTab,
  reactions: ThoughtReactionsTab,
}

const activeTab = ref('comments')
const activeReactionId = ref(null)

function setActiveTab(tab) {
  activeTab.value = tab
}

function setActiveTabWithReactionId(tab, reactionId) {
  setActiveTab(tab)

  activeReactionId.value = reactionId
}

provide('activeReactionId', activeReactionId)
</script>

<template>
  <div
    class="modal fade"
    id="activity-modal"
    tabindex="-1"
    aria-labelledby="activity-modal-label"
    aria-hidden="true"
  >
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
      <div class="modal-content">
        <div class="modal-header">
          <button @click="setActiveTab('comments')" type="button" class="btn btn-dark">
            Comments
          </button>

          <button
            v-for="reaction in focusedThought?.reactions"
            :key="reaction.id"
            @click="setActiveTabWithReactionId('reactions', reaction.id)"
            type="button"
            class="btn btn-dark mx-2"
          >
            <i :class="reactionTypes[reaction.id]"></i>
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
