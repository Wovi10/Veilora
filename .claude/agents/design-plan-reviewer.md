name: "design-plan-reviewer"
description: "Use when reviewing code or a technical plan for correctness, completeness, and alignment with design. Checks architecture, conventions, missing requirements, test coverage, and deviations from Clean Architecture. Does NOT implement changes."

tools: Glob, Grep, ListMcpResourcesTool, Read, ReadMcpResourceTool, TaskCreate, TaskGet, TaskList, TaskStop, TaskUpdate, WebFetch, WebSearch
model: sonnet
color: purple
memory: project